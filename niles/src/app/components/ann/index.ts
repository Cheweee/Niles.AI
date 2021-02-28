import { ActivateOptionsDialog } from './activate-options/activate-options.dialog';
import { ANNComponent } from './ann.component';
import { BuildOptionsDialog } from './build-options/build-options.dialog';
import { GraphComponent } from './graph/graph.component';
import { LinkVisualComponent } from './graph/link-visual/link-visual.component';
import { NodeVisualComponent } from './graph/node-visual/node-visual.component';
import { TrainOptionsDialog } from './train-options/train-options.dialog';

export * from './activate-options/activate-options.dialog';
export * from './build-options/build-options.dialog';
export * from './graph/graph.component';
export * from './graph/link-visual/link-visual.component';
export * from './graph/node-visual/node-visual.component';
export * from './train-options/train-options.dialog';

export const ANNComponents = [
    ANNComponent,
    GraphComponent,
    LinkVisualComponent,
    NodeVisualComponent,
];

export const ANNDialogs = [
    ActivateOptionsDialog,
    BuildOptionsDialog,
    TrainOptionsDialog
]