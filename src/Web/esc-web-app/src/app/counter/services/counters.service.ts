import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiResponse } from '../../shared/models/api-response';
import { Counter } from '../models/counter';

@Injectable({
  providedIn: 'root'
})
export class CountersService {

  constructor(
    private _httpClient: HttpClient
  ) {
  }

  getAll(size: number, lastEntityId?: string): Observable<Counter[]> {
    const afterParameter = lastEntityId
      ? `&after=${encodeURIComponent(lastEntityId)}`
      : '';

    return this._httpClient
      .get<ApiResponse<Counter[]>>(`http://localhost:5000/api/counters?size=` + size + afterParameter)
      .pipe(
        map(resp => {
          if (resp.ok) {
            return resp.value;
          } else {
            throw new Error(resp.message);
          }
        })
      );
  }
}
