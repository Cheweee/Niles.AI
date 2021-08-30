import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NeuralNetworkBuildOptions, NeuralNetworkTrainOptions, NeuralNetworkActivateOptions, NeuralNetwork } from '../models';

@Injectable({ providedIn: 'root' })
export class NeuralNetworkService {
    private readonly _apiUrl = 'api/neuralnetwork';

    private _httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(
        private readonly _httpClient: HttpClient
    ) { }

    public getInstance() : Promise<NeuralNetwork> {
        return this._httpClient.get<NeuralNetwork>(this._apiUrl).toPromise();
    }

    public buildInstance(options: NeuralNetworkBuildOptions): Promise<any>
    {
        return this._httpClient.put(this._apiUrl, JSON.stringify(options), this._httpOptions).toPromise();
    }

    public trainInstance(options: NeuralNetworkTrainOptions): Promise<any>
    {
        return this._httpClient.post(`${this._apiUrl}/train`, options, this._httpOptions).toPromise();
    }

    public activateInstance(options: NeuralNetworkActivateOptions): Promise<any>
    {
        return this._httpClient.post(this._apiUrl, options, this._httpOptions).toPromise();
    }

    public clearInstance(): Promise<any>
    {
        return this._httpClient.delete(this._apiUrl, this._httpOptions).toPromise();
    }

    public getColorFromWeight(pulse: number) {
      const red = 125 * (1 + pulse);
      const green = 200 / (1 + pulse);
      const blue = 255 / (1 + Math.abs(pulse));
  
      return `rgb(${red}, ${green}, ${blue})`;
    }
}