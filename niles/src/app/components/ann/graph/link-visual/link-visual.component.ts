import { Component, Input, AfterViewInit } from '@angular/core';
import { Link, Synapse } from 'src/app/models';

@Component({
  selector: '[linkVisual]',
  template: `
    <svg:line [matTooltip]="tooltip"
        class="link"
        [attr.stroke-width]="link.stroke"
        [attr.stroke]="link.color"
        [attr.x1]="link.source.x"
        [attr.y1]="link.source.y"
        [attr.x2]="link.target.x"
        [attr.y2]="link.target.y"
    ></svg:line>
  `
})
export class LinkVisualComponent implements AfterViewInit  {
  @Input('linkVisual') link: Link<Synapse>;

  tooltip=""

  ngAfterViewInit() {
    if(this.link.data)
      this.tooltip += this.link.data.weight;
  }
}
