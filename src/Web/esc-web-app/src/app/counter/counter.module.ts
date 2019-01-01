import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { CountersService } from './services/counters.service';
import { NewCounterComponent } from './components/new-counter/new-counter.component';
import { AllCountersComponent } from './components/all-counters/all-counters.component';
import { CounterCardComponent } from './components/counter-card/counter-card.component';
import { SingleCounterComponent } from './components/single-counter/single-counter.component';

const routes: Routes = [
  {path: ':name', component: SingleCounterComponent},
  {path: '', component: AllCountersComponent, pathMatch: 'full'},
];

@NgModule({
  declarations: [
    AllCountersComponent,
    CounterCardComponent,
    NewCounterComponent,
    SingleCounterComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    FormsModule,
    ReactiveFormsModule,

    MatGridListModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
  ],
  providers: [
    CountersService
  ]
})
export class CounterModule {
}
