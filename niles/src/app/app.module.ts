import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { D3_DIRECTIVES } from './directives';

import { FlexLayoutModule } from '@angular/flex-layout';

import { AppComponent } from './app.component';

import { ApiUrlInterceptor } from './interceptors';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { MenuComponent } from './components/menu/menu.component';
import { ANNComponents, ANNDialogs } from './components/ann';
import { RouterModule, Routes } from '@angular/router';
import { ANNComponent } from './components/ann/ann.component';
import { SimulatorComponent } from './components/simulator/simulator.component';
import { SimulatorComponents } from './components/simulator';

const routes: Routes = [
  { path: 'ann', component: ANNComponent },
  { path: 'simulator', component: SimulatorComponent },
  { path: '*', redirectTo: 'ann' }
]

@NgModule({
  declarations: [
    AppComponent,
    MenuComponent,
    ...ANNComponents,
    ...ANNDialogs,
    ...SimulatorComponents,
    ...D3_DIRECTIVES
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MaterialModule,
    ReactiveFormsModule,
    FlexLayoutModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ApiUrlInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    ...ANNDialogs,
  ]
})
export class AppModule { }
