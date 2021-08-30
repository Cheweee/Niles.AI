import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormControl, Validators, FormArray } from '@angular/forms';
import { NeuralNetworkTrainOptions, TrainSet, TrainStrategies } from 'src/app/models';

@Component({
    selector: 'train-options',
    templateUrl: './train-options.dialog.html'
})
export class TrainOptionsDialog {
    constructor(public dialogRef: MatDialogRef<TrainOptionsDialog>,
        @Inject(MAT_DIALOG_DATA) public data: number) { }

    public form: FormGroup;
    public inputs: number[][];
    public epoch: number;
    public learningRate: number;
    public moment: number;
    public expectedOutput: number[];

    public ngOnInit(): void {
        this.form = new FormGroup({
            epoch: new FormControl(1000000, [
                Validators.required,
                Validators.min(1)
            ]),
            learningRate: new FormControl(0.0000003, [
                Validators.min(0.0)
            ]),
            moment: new FormControl(0.0000006, [
                Validators.min(0.0)
            ]),
            inputs: new FormArray([], [
                Validators.minLength(1),
            ]),
            outputs: new FormArray([], [
                Validators.minLength(1),
            ])
        });
    }

    onAddDataSetClick(): void {
        const inputsArray = this.form.controls.inputs as FormArray;
        const outputsArray = this.form.controls.outputs as FormArray;

        const newFormControl = new FormControl('');
        inputsArray.push(newFormControl);        
        const outputFormControl = new FormControl('');
        outputsArray.push(outputFormControl);        
    }

    onDeleteDataSetClick(index: number): void {
        const inputsArray = this.form.controls.inputs as FormArray;
        const outputsArray = this.form.controls.outputs as FormArray;
        inputsArray.removeAt(index);
        outputsArray.removeAt(index);
    }

    public onAcceptClick(): void {
        const inputsArray = this.form.controls.inputs as FormArray;
        const outputsArray = this.form.controls.outputs as FormArray;
        const trainSets: TrainSet[] = [];
        for(let i = 0; i < inputsArray.controls.length; i++) {
            const inputControl = inputsArray.controls[i];
            const outputControl = outputsArray.controls[i];
            const tempInput: number[] = this.mapNumberArrayFromString(inputControl.value);
            const tempOutput: number[] = this.mapNumberArrayFromString(outputControl.value);
            trainSets.push({ idealOutput: tempOutput, input: tempInput });
        }
        const options: NeuralNetworkTrainOptions = { 
            epoch: this.form.controls.epoch.value,
            trainSets: trainSets,
            learningRate: this.form.controls.learningRate.value,
            moment: this.form.controls.moment.value,
            trainStrategy: TrainStrategies.BackPropagation
        };

        this.dialogRef.close(options);
    }

    private mapNumberArrayFromString(value: string): number[]
    {
        let result = value.split(', ').map(o => parseInt(o));
        result = result.filter(o => !isNaN(o));

        return result;
    }
}