import { Component, Input, OnInit } from '@angular/core';
import { Counter } from '../../models/counter';

@Component({
  selector: 'app-counter-card',
  templateUrl: './counter-card.component.html',
  styleUrls: ['./counter-card.component.css']
})
export class CounterCardComponent implements OnInit {
  @Input() counter: Counter;

  constructor() {
  }

  ngOnInit() {
  }

}
