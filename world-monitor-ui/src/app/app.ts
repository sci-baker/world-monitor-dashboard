import { Component, ViewChild } from '@angular/core';
import { HeaderComponent } from './components/header/header.component';
import { NewsPanelComponent } from './components/news-panel/news-panel.component';
import { BriefPanelComponent } from './components/brief-panel/brief-panel.component';
import { MapPanelComponent } from './components/map-panel/map-panel.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [HeaderComponent, NewsPanelComponent, BriefPanelComponent, MapPanelComponent],
  template: `
    <app-header (ingested)="onIngested()"></app-header>
    <main class="dashboard">
      <section class="col-news">
        <app-news-panel #newsPanel></app-news-panel>
      </section>
      <section class="col-right">
        <div class="row-brief">
          <app-brief-panel></app-brief-panel>
        </div>
        <div class="row-map">
          <app-map-panel></app-map-panel>
        </div>
      </section>
    </main>
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }
    .dashboard {
      flex: 1;
      display: grid;
      grid-template-columns: 1fr 1.4fr;
      gap: 12px;
      padding: 12px;
      min-height: 0;
      overflow: hidden;
    }
    .col-news {
      overflow-y: auto;
      min-height: 0;
    }
    .col-right {
      display: flex;
      flex-direction: column;
      gap: 12px;
      min-height: 0;
    }
    .row-brief {
      flex: 0 1 auto;
      max-height: 45%;
      overflow-y: auto;
    }
    .row-map {
      flex: 1;
      min-height: 300px;
    }
  `]
})
export class AppComponent {
  @ViewChild('newsPanel') newsPanel!: NewsPanelComponent;

  onIngested(): void {
    this.newsPanel.load();
  }
}
