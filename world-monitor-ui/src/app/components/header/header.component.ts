import { Component, EventEmitter, Output } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
    selector: 'app-header',
    standalone: true,
    template: `
    <header class="dashboard-header">
      <div class="header-left">
        <h1>⌘ World Monitor</h1>
        <span class="subtitle">Intelligence Dashboard</span>
      </div>
      <div class="header-right">
        <button
          class="btn btn-ingest"
          (click)="onIngest()"
          [disabled]="ingesting">
          {{ ingesting ? 'Ingesting…' : '↻ Ingest Feeds' }}
        </button>
      </div>
    </header>
  `,
    styles: [`
    .dashboard-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 24px;
      background: #0f1923;
      border-bottom: 1px solid #1e2d3d;
    }
    .header-left {
      display: flex;
      align-items: baseline;
      gap: 12px;
    }
    h1 {
      margin: 0;
      font-size: 20px;
      color: #e0e6ed;
      font-weight: 600;
    }
    .subtitle {
      font-size: 13px;
      color: #5a7a9a;
      text-transform: uppercase;
      letter-spacing: 1px;
    }
    .btn {
      padding: 8px 16px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 13px;
      font-weight: 500;
    }
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .btn-ingest {
      background: #1a73e8;
      color: #fff;
    }
    .btn-ingest:hover:not(:disabled) {
      background: #1557b0;
    }
  `]
})
export class HeaderComponent {
    @Output() ingested = new EventEmitter<void>();

    ingesting = false;

    constructor(private api: ApiService) { }

    onIngest(): void {
        this.ingesting = true;
        this.api.triggerIngest().subscribe({
            next: () => {
                this.ingesting = false;
                this.ingested.emit();
            },
            error: () => {
                this.ingesting = false;
            }
        });
    }
}
