import { Component } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-new-counter',
  templateUrl: './new-counter.component.html',
  styleUrls: ['./new-counter.component.css']
})
export class NewCounterComponent {

  counterName = new FormControl('', [
    Validators.required,
    Validators.minLength(2),
    Validators.pattern(/^[A-Z](?:[A-Z]|\d|-|_|\.)+$/i),
  ]);

  getErrorMessage() {
    let message: string;
    if (this.counterName.hasError('required')) {
      message = 'Counter name is required';
    } else if (this.counterName.hasError('minlength')) {
      message = 'Not long enough';
    } else if (this.counterName.hasError('pattern')) {
      message = 'Allowed characters are alphanumeric characters, dot, underscore, and hyphen. ' +
        'Name must start with a letter.';
    } else {
      message = 'Error!';
    }
    return message;
  }

  submit() {

  }
}
