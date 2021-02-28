import { namespace } from 'd3';

export interface NeuralNetwork {
    layers: NeuralLayer[];
    readonly output: number[];
    readonly trainEnded: boolean;
}

export interface NeuralLayer {
    id: number;
    name: string;
    neurons: Neuron[];
}

export interface Neuron {
    id: number;
    dendrites: Synapse[];
    axon: number;
    isHidden: boolean;
}

export interface Synapse {
    input: Neuron;
    weight: number;
}

export interface Pulse {
    value: number;
}

export interface NeuralLayerBuildOptions {
    name: string;
    neuronsCount: number;
}

export interface NeuralNetworkBuildOptions {
    layersBuildOptions: NeuralLayerBuildOptions[];
}

export interface NeuralNetworkTrainOptions {
    epoch?: number;
    learningRate?: number;
    moment?: number;
    trainSets?: TrainSet[];
    activateFunction: ActivateFunctions;
    trainStrategy: TrainStrategies;
}

export interface NeuralNetworkActivateOptions {
    input: number[];
    activateFunction: ActivateFunctions;
}

export interface TrainSet {
    input: number[];
    idealOutput: number[];
}

export enum ActivateFunctions {
    Sigmoid,
    HyperbolicTangent
}

export enum TrainStrategies {
    BackPropagation,
    QLearning
}