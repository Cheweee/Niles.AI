import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormControl } from '@angular/forms';
import { ActivateFunctions, NeuralNetworkActivateOptions } from 'src/app/models';

@Component({
    selector: 'activate-options',
    templateUrl: './activate-options.dialog.html'
})
export class ActivateOptionsDialog {
    constructor(public dialogRef: MatDialogRef<ActivateOptionsDialog>,
        @Inject(MAT_DIALOG_DATA) public data: number) { }

    public form: FormGroup;

    public ngOnInit(): void {
        this.form = new FormGroup({
            input: new FormControl('')
        });
    }

    public onAcceptClick(): void {
        const input = this.mapNumberArrayFromString(this.form.controls.input.value);

        const options: NeuralNetworkActivateOptions = {
            input: input
        };

        this.dialogRef.close(options);
    }

    private mapNumberArrayFromString(value: string): number[] {
        let result = value.split(', ').map(o => parseInt(o));
        result = result.filter(o => !isNaN(o));

        return result;
    }
}