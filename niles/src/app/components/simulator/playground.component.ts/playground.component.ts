import { ThrowStmt } from '@angular/compiler';
import { AfterContentInit, ElementRef, EventEmitter, Output } from '@angular/core';
import { HostListener } from '@angular/core';
import { Component, Input, ViewChild, AfterViewInit } from '@angular/core';
import { clear } from 'console';
import { clearLine } from 'readline';
import { Eat, Environment, Infusory } from 'src/app/models/environment';
import { SvgOptions } from 'src/app/models/svgOptions';


function randomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min)) + min; //Максимум не включается, минимум включается
}

@Component({
    selector: 'playground',
    templateUrl: './playground.component.html'
})
export class PlaygroundComponent  {
    @Input('environment') environment: Environment;
    @Input('availableLength') availableLength: number;
    @Input('showCellVision') showCellVision: boolean;

    @Output() checkEnvironment: EventEmitter<null> = new EventEmitter();

    @ViewChild('playground') playground: ElementRef<HTMLCanvasElement>;
    @ViewChild('playgroundContainer') playgroundContainer: ElementRef<HTMLDivElement>;

    public _options: SvgOptions = { width: 1024, height: 860 };
    private _context: CanvasRenderingContext2D;

    private eatInterval: NodeJS.Timeout;
    private environmentInterval: NodeJS.Timeout;

    ngOnInit() {
        this.eatInterval = setInterval(() => {
            const cell: Eat = new Eat(
                randomInt(0, this._options.height),
                randomInt(0, this._options.width),
                10
            );
            this.environment.eat.push(cell);

            if (this.environment.eat.length > this.availableLength) {
                this.environment.eat.splice(0, 1);
            }

            for (let cell of this.environment.infusories) {
                if (cell.r > 10)
                    cell.r -= cell.r / 100;
            }
        }, 5000);

        this.environmentInterval = setInterval(() => {
            this.checkEnvironment.emit();
        }, 500);
    }

    ngAfterViewInit() {
        this.setOptions();
        const canvas = this.playground.nativeElement;
        this._context = canvas.getContext('2d');

        window.requestAnimationFrame(() => this.draw());
    }

    private drawEat(cell: Eat) {
        this._context.beginPath();

        this._context.arc(
            cell.x,
            cell.y,
            cell.r,
            0,
            Math.PI * 2,
            true
        );

        this._context.fillStyle = cell.color();
        this._context.closePath();
        this._context.fill();
    }

    private drawCell(cell: Infusory) {
        const sidesCount = cell.sensors.length;
        this._context.beginPath();

        this._context.moveTo(cell.x + cell.r * Math.cos(0), cell.y + cell.r * Math.sin(0));

        for (let side = 0; side < sidesCount; side++) {
            this._context.lineTo(cell.x + cell.r * Math.cos(side * 2 * Math.PI / sidesCount), cell.y + cell.r * Math.sin(side * 2 * Math.PI / sidesCount));
        }

        this._context.fillStyle = cell.color();

        this._context.closePath();
        this._context.fill();

        if (this.showCellVision) {
            for (let side = 0; side < cell.sensors.length; side++) {
                const sensor = cell.sensors[side];
                this._context.beginPath()
                this._context.moveTo(cell.x +  cell.r * Math.cos(side * 2 * Math.PI / sidesCount), cell.y + cell.r * Math.sin(side * 2 * Math.PI / sidesCount));
                this._context.lineTo(cell.x + cell.r * Math.cos(side * 2 * Math.PI / sidesCount) * 5, cell.y + cell.r * Math.sin(side * 2 * Math.PI / sidesCount) * 5);
                this._context.strokeStyle = sensor.isActivated ? '#ef767a' : '#a7a7a7';
                this._context.lineWidth = sensor.isActivated ? 2 : 1;
                this._context.stroke();
            }
        }
    }

    draw() {
        this._context.clearRect(0, 0, this._options.width, this._options.height);

        for (const eat of this.environment.eat) {
            this.drawEat(eat);
        }

        for (const cell of this.environment.infusories) {
            this.drawCell(cell);
        }

        for (const cell of this.environment.infusories) {
            this.checkCellEnvironment(cell);
        }

        window.requestAnimationFrame(() => this.draw());
    }

    ngOnDestroy() {
        clearInterval(this.eatInterval);
        clearInterval(this.environmentInterval);
    }
    @HostListener('window:resize', ['$event'])
    onResize(event) {
        this.setOptions();
    }

    setOptions() {
        //FIXME: Лютый костыляка
        this._options = {
            width: this.playgroundContainer.nativeElement.scrollWidth,
            height: this.playgroundContainer.nativeElement.scrollHeight
        };
    }

    private checkCellEnvironment(infusory: Infusory) {
        const nearsetEat = this.environment.eat.filter(o => {
            let distSq = Math.sqrt(
                ((infusory.x - o.x) * (infusory.x - o.x)) +
                ((infusory.y - o.y) * (infusory.y - o.y))
            );

            if (o.r + distSq <= infusory.r * 5)
                return o;
        });

        for (const eat of nearsetEat) {
            const distSq = Math.sqrt(Math.pow(eat.x - infusory.x, 2) + Math.pow(eat.y - infusory.y, 2));
            const distR = eat.r + infusory.r;

            if (distSq <= distR)
                this.eat(infusory, eat);
        }

        for (let i = 0; i < infusory.sensors.length; i++) {
            const sensor = infusory.sensors[i];
            const x1 = infusory.x + infusory.r * Math.cos(i * 2 * Math.PI / infusory.sensors.length);
            const y1 = infusory.y + infusory.r * Math.sin(i * 2 * Math.PI / infusory.sensors.length);
            const x2 = infusory.x + infusory.r * Math.cos(i * 2 * Math.PI / infusory.sensors.length) * 5;
            const y2 = infusory.y + infusory.r * Math.sin(i * 2 * Math.PI / infusory.sensors.length) * 5;

            for (const eat of nearsetEat) {
                const ax = x1 - eat.x;
                const ay = y1 - eat.y;
                const bx = x2 - eat.x;
                const by = y2 - eat.y;

                const a = Math.pow(bx - ax, 2) + Math.pow(by - ay, 2);
                const b = 2 * (ax * (bx - ax) + ay * (by - ay));
                const c = Math.pow(ax, 2) + Math.pow(ay, 2) - Math.pow(eat.r, 2);
                const disc = Math.pow(b, 2) - 4 * a * c;

                if (disc < 0) {
                    sensor.isActivated = false;
                    continue;
                }

                const t1 = (-b + Math.sqrt(disc)) / (2 * a);
                const t2 = (-b - Math.sqrt(disc)) / (2 * a);

                sensor.isActivated = (0 < t1 && t1 < 1) || (0 < t2 && t2 < 1);
                if (sensor.isActivated)
                    break;
            }
        }
    }

    eat(cell: Infusory, eat: Eat) {
        cell.r += 1;
        this.environment.eat = this.environment.eat.filter(o => o != eat);
    }

    getPosition(el) {
        var xPosition = 0;
        var yPosition = 0;

        while (el) {
            xPosition += (el.offsetLeft - el.scrollLeft + el.clientLeft);
            yPosition += (el.offsetTop - el.scrollTop + el.clientTop);
            el = el.offsetParent;
        }
        return {
            x: xPosition,
            y: yPosition
        };
    }
}