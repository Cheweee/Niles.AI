import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Link, NeuralNetwork, Synapse, Node, NeuralNetworkBuildOptions, NeuralNetworkTrainOptions, NeuralNetworkActivateOptions } from 'src/app/models';
import { SignalrService, NeuralNetworkService } from 'src/app/services';
import { ActivateOptionsDialog } from './activate-options/activate-options.dialog';
import { BuildOptionsDialog } from './build-options/build-options.dialog';
import { TrainOptionsDialog } from './train-options/train-options.dialog';

@Component({
    selector: 'ann',
    templateUrl: './ann.component.html'
})
export class ANNComponent {
    nodes: Node[] = [];
    links: Link[] = [];
    private _networkInstance: NeuralNetwork;
  
    ngOnInit() {
      this.signalrService.registerReceiveEvent((message) => {
        const networkInstance = JSON.parse(message);
        this._networkInstance = networkInstance;
        this.mapInstanceToGraph(networkInstance);
      });
      // Такой финт нужен, потому что SignalR может не успеть установить соединение
      this.signalrService.startConnection().finally(() => this.getInstance());
    }
  
    public getInstance() {
      this.neuralNetworkService.getInstance();
    }

    public clearInstance() {
      this.neuralNetworkService.clearInstance();
    }
  
    private mapInstanceToGraph(networkInstance: NeuralNetwork) {
      if (!networkInstance || !networkInstance.layers) {
        this.nodes = [];
        this.links = [];
        return;
      }
  
      const neurons = networkInstance.layers
        .map(o => o.neurons)
        .reduce((prev, curr) => prev.concat(curr), []);
      const nodes: Node[] = neurons
        .map((o, index) => {
          return new Node(o.id, o);
        });
      this.nodes = nodes;
  
      const links: Link[] = [];
  
      for (let i = networkInstance.layers.length - 1; i > 0; i--) {
        const layer = networkInstance.layers[i];
  
        for (let neuron of layer.neurons) {
          for (let dendrite of neuron.dendrites) {
            const nodeFrom = nodes.find(n => n.id === neuron.id);
            const nodeTo = nodes.find(n => n.id === dendrite.input.id);
  
            nodeFrom.linkCount++;
            nodeTo.linkCount++;
  
            const link = new Link<Synapse>(nodeFrom, nodeTo, 1, this.neuralNetworkService.getColorFromWeight(dendrite.weight), dendrite);
  
            links.push(link);
          }
        }
      }
      this.links = links;
    }
  
    public onBuildInstanceClick() {
      const dialogRef = this.dialog.open(BuildOptionsDialog);
  
      dialogRef.afterClosed().subscribe((result: NeuralNetworkBuildOptions) => {
        if (!result) return;
  
        this.neuralNetworkService.buildInstance(result);
      });
    }
  
    public onTrainInstanceClick() {
      const inputNeuronsCount = this._networkInstance.layers[0].neurons.length;
      const dialogRef = this.dialog.open(TrainOptionsDialog, {
        data: inputNeuronsCount
      });
  
      dialogRef.afterClosed().subscribe((result: NeuralNetworkTrainOptions) => {
        if (!result) return;
  
        this.neuralNetworkService.trainInstance(result);
      });
    }
  
    public onActivateInstanceClick() {
      const inputNeuronsCount = this._networkInstance.layers[0].neurons.length;
      const dialogRef = this.dialog.open(ActivateOptionsDialog, {
        data: inputNeuronsCount
      });
  
      dialogRef.afterClosed().subscribe((result: NeuralNetworkActivateOptions) => {
        if (!result) return;
  
        this.neuralNetworkService.activateInstance(result);
      });
    }
  
    constructor(
      private readonly signalrService: SignalrService,
      private readonly neuralNetworkService: NeuralNetworkService,
      private dialog: MatDialog) { }
}