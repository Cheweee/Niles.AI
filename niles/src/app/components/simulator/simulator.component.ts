import { compileNgModule, ThrowStmt } from '@angular/compiler';
import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { clearLine } from 'readline';
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
    private readonly _infusorySpeed: number = 2;

    public environment: Environment;
    public environmentDensity: number = this._initialEnvironmentDensity;
    public availableLength: number = 0;
    public showVision: boolean = false;
    public isManual: boolean = false;
    public sensorsCount: number = this._initialSensorsCount;
    public simulationStarted: boolean = false;
    public paused: boolean = false;
    @ViewChild('playgroundContainer') playgroundContainer: PlaygroundComponent;

    constructor(
        private readonly _neuralNetworkService: NeuralNetworkService,
        private readonly _signalrService: SignalrService
    ) {
        this.environment = new Environment();
    }

    ngOnInit() {
        this._signalrService.registerReceiveEvent((message) => this.getANNOutput(message));
    }

    ngAfterViewInit() {
        this.initializeFood();
    }

    private getANNOutput(message) {
        const networkInstance: NeuralNetwork = JSON.parse(message);

        if (networkInstance.output.length !== 4)
            throw new Error('Ошибка настроек нейронной сети');

        console.log(`[${networkInstance.output.map(o => o.toString()).reduce((p, c) => `${p}, ${c}`)}]`);

        for (const cell of this.environment.infusories) {

            console.log(`mass is: ${cell.r}`);

            if(networkInstance.output[0] != 0)
            cell.x += networkInstance.output[0] * this._infusorySpeed;
            if(networkInstance.output[2] != 0)
            cell.x -= networkInstance.output[2] * this._infusorySpeed;

            
            if(networkInstance.output[1] != 0)
            cell.y += networkInstance.output[1] * this._infusorySpeed;
            if(networkInstance.output[3] != 0)
            cell.y -= networkInstance.output[3] * this._infusorySpeed;
        }
    }

    public onCheckEnvironment() {
        if (this.isManual)
            return;

        for (const cell of this.environment.infusories) {
            this._neuralNetworkService.activateInstance({
                input: cell.sensors.map(o => o.isActivated ? 1 : 0)
            });
        }
    }

    public async start() {
        if (this.paused) {
            await this._signalrService.startConnection();
            await this._neuralNetworkService.getInstance();
            this.paused = false;
            return;
        }
        this.simulationStarted = true;
        this.initializeInfusories();
    }

    public pause() {
        this._signalrService.stopConnection();
        this.paused = true;
    }

    public stop(): void {
        this.sensorsCount = this._initialSensorsCount;
        this.simulationStarted = false;
        this.isManual = false;

        this.clearInfusories();
        this._signalrService.stopConnection();
    }

    private getAvailableLenght(): number {

        const square = this.playgroundContainer._options.width * this.playgroundContainer._options.height;

        const availableDensity = square / 100;

        return availableDensity * this.environmentDensity / 100 / 100;
    }

    private initializeFood() {
        this.environment.eat = [];

        this.availableLength = this.getAvailableLenght();

        for (let i = 0; i < this.availableLength; i++) {
            this.environment.eat.push(new Eat(
                randomInt(0, this.playgroundContainer._options.width),
                randomInt(0, this.playgroundContainer._options.height),
                10
            ));
        }
    }

    private async clearInfusories() {
        this.environment.infusories = [];
        if (this.isManual)
            return;

        await this._neuralNetworkService.clearInstance();
    }

    private async initializeInfusories() {
        this.clearInfusories();
        this.environment.infusories.push(new Infusory(
            randomInt(0, this.playgroundContainer._options.width),
            randomInt(0, this.playgroundContainer._options.height),
            30,
            this.sensorsCount
        ));

        if (this.isManual) {
            return;
        }

        // Такой финт нужен, потому что SignalR может не успеть установить соединение
        this._signalrService.startConnection().finally(async () => await this._neuralNetworkService.getInstance());

        await this._neuralNetworkService.buildInstance({
            layersBuildOptions: [
                { name: 'Sensors', neuronsCount: this.sensorsCount },
                { name: 'HiddenLayer_1', neuronsCount: 8 },
                { name: 'HiddenLayer_2', neuronsCount: 8 },
                { name: 'HiddenLayer_2', neuronsCount: 8 },
                { name: 'Controls', neuronsCount: 4 }
            ],
            activateFunction: ActivateFunctions.HyperbolicTangent
        });
    }

    public slideEnd() {
        this.availableLength = this.getAvailableLenght();
        if (this.availableLength > this.environment.eat.length) {
            for (let i = this.environment.eat.length; i < this.availableLength; i++)
                this.environment.eat.push(new Eat(
                    randomInt(0, this.playgroundContainer._options.width),
                    randomInt(0, this.playgroundContainer._options.height),
                    10
                ));
        } else {
            for (let i = this.environment.eat.length; i > this.availableLength; i--)
            this.environment.eat.splice(randomInt(0, this.environment.eat.length), 1);
        }
    }

    // @HostListener('document:keypress', ['$event'])
    // mouseMoved(event: MouseEvent) {
    //     if (!this.isManual)
    //         return;

    //     for (const cell of this.environment.infusories) {
    //         if (event.movementX < 0)
    //             cell.x -= this._infusorySpeed;
    //         if (event.movementX > 0)
    //             cell.x += this._infusorySpeed;
    //         if (event.movementY < 0)
    //             cell.y -= this._infusorySpeed;
    //         if (event.movementY > 0)
    //             cell.y += this._infusorySpeed;
    //     }

    // }
}