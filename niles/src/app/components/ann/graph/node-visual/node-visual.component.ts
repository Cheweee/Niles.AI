import { Component, Input } from '@angular/core';
import { Neuron, Node } from 'src/app/models';

@Component({
  selector: '[nodeVisual]',
  template: `
    <svg:g [matTooltip]="tooltip" [attr.transform]="'translate(' + node.x + ',' + node.y + ')'">
      <svg:circle
          class="node"
          [attr.fill]="node.data.isHidden ? '#a5a5a5' : node.color"
          cx="0"
          cy="0"
          [attr.r]="node.r">
      </svg:circle>
      <svg:text
          class="node-name"
          [attr.font-size]="node.fontSize">
        {{(node.data.isHidden ? 'H' : '') + node.id}}
      </svg:text>
    </svg:g>
  `,
  styleUrls: ['./node-visual.component.css']
})
export class NodeVisualComponent {
  @Input('nodeVisual') node: Node<Neuron>;

  tooltip = "";

  ngAfterViewInit() {
    if(this.node.data)
      this.tooltip += this.node.data.axon;
  }
}
