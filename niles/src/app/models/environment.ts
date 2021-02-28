export class Environment {
    public infusories: Infusory[] = [];

    public eat: Eat[] = [];
}

abstract class Cell {
    public x: number = 0;
    public y: number = 0;
    public r: number = 30;

    public abstract color();

    constructor(x: number, y: number, r: number) {
        this.x = x;
        this.y = y;
        this.r = r;
    }
}

export class Eat extends Cell {
    public color() { return '#23f0c7' };

    constructor(x: number, y: number, r: number) {
        super(x, y, r);
    }
}

export class Sensor {
    public isActivated: boolean;
}

export class Infusory extends Cell {
    public color() { return '#6457a6' };
    public sensors: Sensor[] = [];

    constructor(x: number, y: number, r: number, sensorsCount: number) {
        super(x, y, r);
        for (let i = 0; i < sensorsCount; i++)
            this.sensors.push({ isActivated: false });
    }
}