import { Component, Input, ChangeDetectorRef, HostListener, ChangeDetectionStrategy, OnInit, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { ForceDirectedGraph } from 'src/app/models';
import { SvgOptions } from 'src/app/models/svgOptions';
import { D3Service } from 'src/app/services';

@Component({
  selector: 'graph',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="h-100" #svgContainer id="svgContainer">
    <svg #svg [attr.width]="_options.width" [attr.height]="_options.height">
      <g [zoomableOf]="svg">
        <g [linkVisual]="link" *ngFor="let link of links"></g>
        <g [nodeVisual]="node" *ngFor="let node of nodes"
            [draggableNode]="node" [draggableInGraph]="graph"></g>
      </g>
    </svg>
    </div>
  `
})
export class GraphComponent implements AfterViewInit {
  @Input('nodes') nodes;
  @Input('links') links;
  @ViewChild('svgContainer') svgContainer: ElementRef<HTMLElement>;
  graph: ForceDirectedGraph;
  public _options: SvgOptions = { width: 1024, height: 860 };

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.setOptions();
    this.graph.initSimulation(this._options);
  }

  constructor(private d3Service: D3Service, private ref: ChangeDetectorRef) { }

  ngOnChanges() {
    this.graph = this.d3Service.getForceDirectedGraph(this.nodes, this.links, this._options);

    this.graph.ticker.subscribe((d) => {
      this.ref.markForCheck();
    });
  }

  ngAfterViewInit() {
    this.setOptions();
    this.graph.initSimulation(this._options);
  }

  setOptions() {
    //FIXME: Лютый костыляка
    this._options = {
      ...this._options,
      width: this.svgContainer.nativeElement.scrollWidth - 18,
      height: this.svgContainer.nativeElement.scrollHeight - 18
    };
  }
}
