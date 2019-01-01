import { Component } from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  hideSidenav: boolean;

  constructor(
    private _mediaObserver: MediaObserver
  ) {
    this._mediaObserver.media$
      .subscribe(
        mediaChange => {
          this.hideSidenav = ['xs', 'sm'].includes(mediaChange.mqAlias);
        }
      );
  }
}
