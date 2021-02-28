import { Node } from './';

export class Link<T = any> implements d3.SimulationLinkDatum<Node> {
  // optional - defining optional implementation properties - required for relevant typing assistance
  index?: number;

  // must - defining enforced implementation properties
  source: Node;
  target: Node;

  stroke?: number = 1;
  color?: string = 'rgb(222,237,250)';

  data: T;

  constructor(source, target, stroke?, color?, data?) {
    this.source = source;
    this.target = target;
    if(stroke) this.stroke = stroke;
    if (color)
      this.color = color;

    this.data = data;
  }
}
