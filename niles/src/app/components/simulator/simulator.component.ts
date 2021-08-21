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
    private readonly _initialEnvironmentDensity: number = 15;
    private readonly _initialSensorsCount: number = 16;

    public environment: Environment;
    public environmentDensity: number = this._initialEnvironmentDensity;
    public availableLength: number;
    public showVision: boolean = false;
    public sensorsCount: number = this._initialSensorsCount;
    @ViewChild('playgroundContainer') playgroundContainer: PlaygroundComponent;

    constructor(
        private _neuralNetworkService: NeuralNetworkService,
        private readonly _signalrService: SignalrService
    ) {
        this.environment = new Environment();
    }

    ngOnInit() {
        this._signalrService.registerReceiveEvent((message) => this.getANNOutput(message));
        // Такой финт нужен, потому что SignalR может не успеть установить соединение
        this._signalrService.startConnection().finally(() => this._neuralNetworkService.getInstance());
    }

    ngAfterViewInit() {
        this.initializeFood();
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
        this.initializeInfusories();
    }

    public stop(): void {
        this.environmentDensity = this._initialEnvironmentDensity;
        this.sensorsCount = this._initialSensorsCount;

        this.clearInfusories();
        this.initializeFood();
    }

    private initializeFood() {
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
    }

    private clearInfusories() {
        this.environment.infusories = [];
        this._neuralNetworkService.clearInstance();
    }

    private initializeInfusories() {
        this.clearInfusories();
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