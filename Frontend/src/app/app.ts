import {inject, Component, computed, signal, WritableSignal} from '@angular/core';
import {TradingApiService} from './TradingApi.service';

interface Instrument {
  securityId: number;
  symbol: string;
  last: number;
  changePercent: number;
}

interface Level {
  price: number;
  quantity: number;
}

interface LadderRow {
  price: number;
  quantity: number;
  cumulative: number;
  depthPercentage: number;
  isBest: boolean;
}

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  readonly dark = signal(typeof window !== 'undefined' && window.matchMedia('(prefers-color-scheme: dark)').matches);
  readonly activeId = signal(1);

  readonly instruments = signal<Instrument[]>([
    {securityId: 1, symbol: 'MSFT', last: 418.05, changePercent: 1.5},
    {securityId: 2, symbol: 'AAPL', last: 227.15, changePercent: -0.18},
    {securityId: 3, symbol: 'NVDA', last: 180.55, changePercent: 2.94},
  ]);

  readonly asks = signal<Level[]>([
    {price: 41810, quantity: 120}, {price: 41815, quantity: 340},
    {price: 41820, quantity: 120}, {price: 41825, quantity: 560},
    {price: 41835, quantity: 150}, {price: 41850, quantity: 700},
  ]);

  readonly bids = signal<Level[]>([
    {price: 41800, quantity: 260}, {price: 41795, quantity: 480},
    {price: 41790, quantity: 190}, {price: 41780, quantity: 620},
    {price: 41770, quantity: 300}, {price: 41755, quantity: 540},
  ]);

  constructor() {
    this.applyTheme();
  }

  toggleTheme(): void {
    this.dark.update((d: boolean) => !d);
    this.applyTheme();
  }

  private applyTheme(): void {
    if (typeof document !== 'undefined') {
      document.documentElement.setAttribute('data-theme', this.dark() ? 'dark' : 'light');
    }
  }

  private readonly maxCumulative = computed(() => {
    const total = (ls: Level[]) => ls.reduce((s, l) => s + l.quantity, 0);
    return Math.max(1, total(this.asks()), total(this.bids()));
  });

  readonly askRows = computed(() => this.toLadder(this.asks()).reverse());
  readonly bidRows = computed(() => this.toLadder(this.bids()));

  private toLadder(levels: Level[]): LadderRow[] {
    const max = this.maxCumulative();
    let cumulative = 0;
    return levels.map((l, i) => {
      cumulative += l.quantity;
      return {
        price: l.price,
        quantity: l.quantity,
        cumulative,
        depthPercentage: (cumulative / max) * 100,
        isBest: i === 0
      };
    });
  }

  readonly bestAsk = computed(() => this.asks()[0]?.price ?? null);
  readonly bestBid = computed(() => this.bids()[0]?.price ?? null);

  readonly spreadCents = computed(() => {
    const a = this.bestAsk(), b = this.bestBid();
    return a !== null && b !== null ? a - b : null;
  });

  readonly mid = computed(() => {
    const a = this.bestAsk(), b = this.bestBid();
    return a !== null && b !== null ? (a + b) / 2 : null;
  });

  select(id: number): void {
    this.activeId.set(id);
  }


// order entry
  readonly side = signal<'buy' | 'sell'>('buy');
  readonly trader = signal('sbanks');
  readonly priceInput = signal('418.50');
  readonly quantityInput = signal('500');
  readonly activeInstrument = computed(() => this.instruments().find(i => i.securityId === this.activeId()) ?? null);
  readonly priceCents = computed(() => Math.round((parseFloat(this.priceInput()) || 0) * 100));
  readonly quantity = computed(() => parseInt(this.quantityInput(), 10) || 0);
  readonly notional = computed(() => (this.priceCents() * this.quantity()) / 100);


  setSide(s: 'buy' | 'sell'): void {
    this.side.set(s);
  }

  /*
    inputValue(e: Event): void {
      return (e.target as HTMLInputElement).value;
    }
  */
  updateInput(targetSignal: WritableSignal<string>, event: Event): void {
    targetSignal.set((event.target as HTMLInputElement).value);
  }

  private readonly api = inject(TradingApiService);
  readonly submitError = signal('');
  readonly isSubmitting = signal(false);

  submit(e: Event): void {
    event?.preventDefault();
    this.submitError.set('');

    if(!this.trader().trim()){
      this.submitError.set("Enter a trader name");
      return;
    }

    if(this.priceCents() <= 0 || this.quantity() <= 0){
      this.submitError.set("Price & Quantity must be greater than 0");
    }

    this.isSubmitting.set(true);

    this.api.placeOrder({
      securityId: this.activeId(),
      username: this.trader().trim(),
      side: this.side() === 'buy'? 'Buy': 'Sell',
      price: this.priceCents(),
      quantity: this.quantity(),
    }).subscribe({
      next: ack => {
        console.log(`Order Accepted`, ack);
        this.isSubmitting.set(false);
      },
      error: () => {
        this.submitError.set(`Order not accepted.`);
        this.isSubmitting.set(false);
      },
    });
  }
}

