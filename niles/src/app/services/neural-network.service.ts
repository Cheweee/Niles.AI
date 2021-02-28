import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NeuralNetworkBuildOptions, NeuralNetworkTrainOptions, NeuralNetworkActivateOptions } from '../models';

@Injectable({ providedIn: 'root' })
export class NeuralNetworkService {
    private readonly _apiUrl = 'api/neuralnetwork';

    private _httpOptions = {
        headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    constructor(
        private readonly _httpClient: HttpClient
    ) { }

    public getInstance() {
        return this._httpClient.get(this._apiUrl).subscribe(result => console.log('success'));
    }

    public buildInstance(options: NeuralNetworkBuildOptions)
    {
        return this._httpClient.put(this._apiUrl, JSON.stringify(options), this._httpOptions).subscribe(result => console.log('success'));
    }

    public trainInstance(options: NeuralNetworkTrainOptions)
    {
        return this._httpClient.post(`${this._apiUrl}/train`, options, this._httpOptions).subscribe(result => console.log('success'));
    }

    public activateInstance(options: NeuralNetworkActivateOptions)
    {
        return this._httpClient.post(this._apiUrl, options, this._httpOptions).subscribe(result => console.log('success'));
    }

    public getColorFromWeight(pulse: number) {
      const red = 125 * (1 + pulse);
      const green = 200 / (1 + pulse);
      const blue = 255 / (1 + Math.abs(pulse));
  
      return `rgb(${red}, ${green}, ${blue})`;
    }
}