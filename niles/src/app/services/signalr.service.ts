import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

@Injectable({ providedIn: 'root' })
export class SignalrService {
    private connection: HubConnection;

    public constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl('http://localhost:5000/neuralnetworkhub')
            .configureLogging(LogLevel.Information)
            .build();        
    }

    public startConnection() {
        return this.connection.start()
            .then(() => console.log('Connection started!'))
            .catch(err => {
                console.log('connection error')
                this.connection.start()
            });
    }

    public registerReceiveEvent = (callback) => {
        this.connection.on("niles.neuralnetwork", function (message) {
            callback(message);
        });
    }
}
