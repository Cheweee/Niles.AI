import { Component } from "@angular/core";
import { MatDialogRef } from '@angular/material/dialog';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { NeuralLayerBuildOptions, NeuralNetworkBuildOptions } from 'src/app/models';

@Component({
    selector: 'build-options',
    templateUrl: './build-options.dialog.html',
})
export class BuildOptionsDialog {
    constructor(public dialogRef: MatDialogRef<BuildOptionsDialog>) { }

    public form: FormGroup;
    public layersBuildOptions: NeuralLayerBuildOptions[];

    public ngOnInit(): void {
        this.layersBuildOptions = [];

        this.form = new FormGroup({

        });
    }

    public onAcceptClick(): void {
        for(let i = 0; i < this.layersBuildOptions.length; i++) {
            const layerNameControl = `layerName${i}`;
            const neuronsCountControl = `neuronsCount${i}`;

            this.layersBuildOptions[i].name = this.form.controls[layerNameControl].value;
            this.layersBuildOptions[i].neuronsCount = parseInt(this.form.controls[neuronsCountControl].value);
        }
        const options: NeuralNetworkBuildOptions = {
            layersBuildOptions: this.layersBuildOptions
        };
        this.dialogRef.close(options);
    }

    public onAddLayerClick(): void {
        this.createLayerOptions();
    }

    public onDeleteLayerOptionsClick(index: number): void {
        this.layersBuildOptions.splice(index, 1);
        const layerNameControl = `layerName${index}`;
        const neuronsCountControl = `neuronsCount${index}`;
        this.form.removeControl(layerNameControl);
        this.form.removeControl(neuronsCountControl);
    }

    private createLayerOptions(): void {
        const layerOptions: NeuralLayerBuildOptions = {
            name: `NEW_LAYER${this.layersBuildOptions.length + 1}`,
            neuronsCount: 1
        };
        this.layersBuildOptions.push(layerOptions);
        const layerNameControl = `layerName${this.layersBuildOptions.length - 1}`;
        const neuronsCountControl = `neuronsCount${this.layersBuildOptions.length - 1}`;
        this.form.addControl(layerNameControl, new FormControl(layerOptions.name, [
            Validators.required
        ]));
        this.form.addControl(neuronsCountControl, new FormControl(layerOptions.neuronsCount, [
            Validators.required
        ]))
    }
}