import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';


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
