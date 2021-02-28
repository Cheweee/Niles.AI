import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivateFunctions, NeuralNetwork, TrainSet, TrainStrategies } from 'src/app/models';
import { Infusory, Environment, Eat } from 'src/app/models/environment';
import { NeuralNetworkService, SignalrService } from 'src/app/services';
import { PlaygroundComponent } from './playground.component.ts/playground.component';


function randomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min; //Максимум не включается, минимум включается
}


@Component({
    selector: 'simulator',
    templateUrl: './simulator.component.html'
})
export class SimulatorComponent {
    public environment: Environment;
    public environmentDensity: number = 15;
    public availableLength: number;
    public showVision: boolean = false;
    public sensorsCount: number = 16;
    @ViewChild('playgroundContainer') playgroundContainer: PlaygroundComponent;

    constructor(private _neuralNetworkService: NeuralNetworkService,
        private readonly signalrService: SignalrService) {
        this.environment = new Environment();
    }

    ngOnInit() {
        this.signalrService.registerReceiveEvent((message) => this.getANNOutput(message));
        // Такой финт нужен, потому что SignalR может не успеть установить соединение
        this.signalrService.startConnection().finally(() => this._neuralNetworkService.getInstance());
    }

    private getANNOutput(message) {
        const networkInstance: NeuralNetwork = JSON.parse(message);

        for (const cell of this.environment.infusories) {

            if (networkInstance.output.length !== 2)
                throw new Error('Ошибка настроек нейронной сети');

            cell.x += networkInstance.output[0] * 2;
            cell.y += networkInstance.output[1] * 2;
        }
    }

    public onCheckEnvironment() {
        for (const cell of this.environment.infusories) {            
            this._neuralNetworkService.activateInstance({
                input: cell.sensors.map(o => o.isActivated ? 1 : 0),
                activateFunction: ActivateFunctions.HyperbolicTangent
            });
        }
    }

    public start() {
        this.environment.infusories = [];
        this.environment.eat = [];

        const square = this.playgroundContainer._options.width * this.playgroundContainer._options.height;

        const availableDensity = square / 100;

        this.availableLength = availableDensity * this.environmentDensity / 100 / 100;

        for (let i = 0; i < this.availableLength; i++) {
            this.environment.eat.push(new Eat(
                randomInt(0, this.playgroundContainer._options.width),
                randomInt(0, this.playgroundContainer._options.height),
                10
            ));
        }

        this.environment.infusories.push(new Infusory(
            randomInt(0, this.playgroundContainer._options.width),
            randomInt(0, this.playgroundContainer._options.height),
            30,
            this.sensorsCount
        ));

        this._neuralNetworkService.buildInstance({
            layersBuildOptions: [
                { name: 'Sensors', neuronsCount: this.sensorsCount },
                { name: 'HiddenLayer_1', neuronsCount: 8 },
                { name: 'HiddenLayer_2', neuronsCount: 8 },
                { name: 'HiddenLayer_2', neuronsCount: 8 },
                { name: 'Controls', neuronsCount: 2 }
            ]
        });
    }
}