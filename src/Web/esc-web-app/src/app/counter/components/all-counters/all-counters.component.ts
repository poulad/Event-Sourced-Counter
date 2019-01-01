import { Component, OnInit } from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';
import { CountersService } from '../../services/counters.service';
import { Counter } from '../../models/counter';

const pageSize = 12;
declare type State = 'loading' | 'loaded';

@Component({
  selector: 'app-all-counters',
  templateUrl: './all-counters.component.html',
  styleUrls: ['./all-counters.component.css']
})
export class AllCountersComponent implements OnInit {
  counters: Counter[];
  numOfCols: number;
  state: State;

  constructor(
    private _mediaObserver: MediaObserver,
    private _countersService: CountersService
  ) {
  }

  ngOnInit() {
    this.counters = new Array(pageSize);
    this.state = 'loading';

    this._countersService
      .getAll(pageSize)
      .subscribe(
        counters => {
          this.counters = counters;
          this.state = 'loaded';
        },
        e => {
          console.warn(e);
        }
      );

    this._mediaObserver.media$
      .subscribe(
        mediaChange => {
          const colsMap = {
            xs: 1,
            sm: 2,
            md: 2,
            lg: 3,
            xl: 4
          };

          this.numOfCols = +colsMap[mediaChange.mqAlias];
        }
      );
  }
}
