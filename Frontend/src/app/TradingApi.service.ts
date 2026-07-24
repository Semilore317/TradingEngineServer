import {HttpClient} from '@angular/common/http';
import {Injectable, inject} from '@angular/core';
import {Observable} from 'rxjs';
import {
  OrderAck,
  PlaceOrderRequest,
} from './trading.models';

@Injectable({providedIn: 'root'})
export class TradingApiService {
  private readonly http = inject(HttpClient);

  placeOrder(order: PlaceOrderRequest): Observable<OrderAck> {
    return this.http.post<OrderAck>('/orders', order);
  }

  cancelOrder(
    securityId: number,
    orderId: number,
    username: string,
  ): Observable<void> {
    return this.http.delete<void>(`/instruments/${securityId}/orders/${orderId}`, {params: {username}});
  }
}
