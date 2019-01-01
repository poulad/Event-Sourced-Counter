import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SingleCounterComponent } from './single-counter.component';

describe('SingleCounterComponent', () => {
  let component: SingleCounterComponent;
  let fixture: ComponentFixture<SingleCounterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SingleCounterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SingleCounterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
