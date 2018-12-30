import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CounterCardComponent } from './counter-card.component';

describe('CounterCardComponent', () => {
  let component: CounterCardComponent;
  let fixture: ComponentFixture<CounterCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CounterCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CounterCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
