import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { Brief } from '../../models/api.models';

@Component({
    selector: 'app-brief-panel',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="panel">
      <div class="panel-header">
        <h2>🧠 AI Intelligence Brief</h2>
        <button
          class="btn btn-generate"
          (click)="generate()"
          [disabled]="generating">
          {{ generating ? 'Generating…' : '⚡ Generate' }}
        </button>
      </div>

      <div class="loading" *ngIf="loading && !generating">Loading brief…</div>
      <div class="generating" *ngIf="generating">
        <div class="spinner"></div>
        <span>Generating brief via AI… this may take a minute.</span>
      </div>
      <div class="error" *ngIf="error">⚠ {{ error }}</div>

      <div class="brief-content" *ngIf="brief && !generating">
        <div class="brief-meta">
          <span class="model">{{ brief.model }}</span>
          <span class="articles">{{ brief.articleCount }} articles analysed</span>
          <span class="date">{{ brief.generatedAt | date:'medium' }}</span>
        </div>
        <h3>{{ brief.title }}</h3>
        <div class="brief-text">{{ brief.content }}</div>
      </div>

      <div class="empty" *ngIf="!loading && !generating && !error && !brief">
        No brief generated yet. Click "Generate" to create one.
      </div>
    </div>
  `,
    styles: [`
    .panel {
      background: #0f1923;
      border: 1px solid #1e2d3d;
      border-radius: 6px;
      padding: 16px;
      overflow-y: auto;
      max-height: 100%;
    }
    .panel-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 12px;
    }
    h2 {
      margin: 0;
      font-size: 15px;
      color: #c8d6e5;
      font-weight: 600;
    }
    .btn {
      padding: 6px 14px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 12px;
      font-weight: 500;
    }
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    .btn-generate {
      background: #27ae60;
      color: #fff;
    }
    .btn-generate:hover:not(:disabled) {
      background: #1e8449;
    }
    .loading, .error, .empty {
      font-size: 13px;
      color: #5a7a9a;
      padding: 20px 0;
      text-align: center;
    }
    .error { color: #e74c3c; }
    .generating {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 20px 0;
      color: #f39c12;
      font-size: 13px;
    }
    .spinner {
      width: 18px;
      height: 18px;
      border: 2px solid #1e2d3d;
      border-top-color: #f39c12;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }
    @keyframes spin {
      to { transform: rotate(360deg); }
    }
    .brief-meta {
      display: flex;
      gap: 10px;
      font-size: 11px;
      margin-bottom: 10px;
    }
    .model {
      background: #6c3483;
      color: #fff;
      padding: 2px 6px;
      border-radius: 3px;
    }
    .articles {
      color: #5a7a9a;
    }
    .date {
      color: #5a7a9a;
      margin-left: auto;
    }
    h3 {
      margin: 0 0 10px 0;
      font-size: 14px;
      color: #e0e6ed;
      font-weight: 600;
    }
    .brief-text {
      font-size: 13px;
      color: #a0b4c8;
      line-height: 1.7;
      white-space: pre-wrap;
    }
  `]
})
export class BriefPanelComponent implements OnInit {
    brief: Brief | null = null;
    loading = false;
    generating = false;
    error = '';

    constructor(private api: ApiService) { }

    ngOnInit(): void {
        this.loadBrief();
    }

    loadBrief(): void {
        this.loading = true;
        this.error = '';
        this.api.getLatestBrief().subscribe({
            next: (data) => {
                this.brief = data;
                this.loading = false;
            },
            error: (err) => {
                // 404 means no brief yet — not an error
                if (err.status === 404) {
                    this.brief = null;
                } else {
                    this.error = 'Failed to load brief';
                }
                this.loading = false;
            }
        });
    }

    generate(): void {
        this.generating = true;
        this.error = '';
        this.api.generateBrief().subscribe({
            next: (data) => {
                this.brief = data;
                this.generating = false;
            },
            error: () => {
                this.error = 'Brief generation failed. Is Ollama running?';
                this.generating = false;
            }
        });
    }
}
