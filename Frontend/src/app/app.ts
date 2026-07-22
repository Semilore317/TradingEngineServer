import { Component, signal } from '@angular/core';

interface Instrument{
  securityId: number;
  symbol: string;
  last: number; // last price in $$$
  changePct: number; // day change %
}

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor() {
  }

  // --sample data for now, real data from the API will come later
  readonly instruments = signal<Instrument[]>([
    {securityId: 1, symbol: 'MSFT', last: 418.05, changePct: 15},
    {securityId: 2, symbol: 'AAPL', last: 227.15, changePct: -0.18},
    {securityId: 3, symbol: 'MSFT', last: 180.55, changePct: 2.94},
  ]);

  readonly activeId = signal(1); // which tab is selected
  readonly dark = signal(matchMedia('(prefers-color-scheme: dark)').matches); // theme

  select(id: number): void{
    this.activeId.set(id);
  }

  toggleTheme(): void{
    this.dark.update(d => !d);
    this.applyTheme();
  }

  private applyTheme(): void{
    document.documentElement.setAttribute('data-theme',this.dark() ? 'dark' : 'light');
  }
}
