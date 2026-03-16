import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { Article } from '../../models/api.models';

@Component({
    selector: 'app-news-panel',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="panel">
      <div class="panel-header">
        <h2>📰 News Feed</h2>
        <span class="count" *ngIf="articles.length">{{ articles.length }} articles</span>
      </div>

      <div class="loading" *ngIf="loading">Loading articles…</div>
      <div class="error" *ngIf="error">⚠ {{ error }}</div>
      <div class="empty" *ngIf="!loading && !error && articles.length === 0">
        No articles yet. Click "Ingest Feeds" to fetch news.
      </div>

      <div class="article-list" *ngIf="articles.length > 0">
        <div class="article-card" *ngFor="let a of articles">
          <div class="article-meta">
            <span class="source">{{ a.sourceName }}</span>
            <span class="category">{{ a.category }}</span>
            <span class="date">{{ a.publishedAt | date:'short' }}</span>
          </div>
          <a class="article-title" [href]="a.url" target="_blank" rel="noopener">
            {{ a.title }}
          </a>
          <p class="article-summary" *ngIf="a.summary">
            {{ a.summary | slice:0:200 }}{{ a.summary.length > 200 ? '…' : '' }}
          </p>
        </div>
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
    .count {
      font-size: 12px;
      color: #5a7a9a;
    }
    .loading, .error, .empty {
      font-size: 13px;
      color: #5a7a9a;
      padding: 20px 0;
      text-align: center;
    }
    .error { color: #e74c3c; }
    .article-list {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }
    .article-card {
      background: #162231;
      border: 1px solid #1e2d3d;
      border-radius: 4px;
      padding: 12px;
    }
    .article-meta {
      display: flex;
      gap: 8px;
      margin-bottom: 6px;
      font-size: 11px;
    }
    .source {
      background: #1a73e8;
      color: #fff;
      padding: 2px 6px;
      border-radius: 3px;
    }
    .category {
      background: #1e2d3d;
      color: #7fa3c9;
      padding: 2px 6px;
      border-radius: 3px;
    }
    .date {
      color: #5a7a9a;
      margin-left: auto;
    }
    .article-title {
      display: block;
      font-size: 14px;
      font-weight: 500;
      color: #e0e6ed;
      text-decoration: none;
      line-height: 1.4;
      margin-bottom: 4px;
    }
    .article-title:hover {
      color: #4da6ff;
    }
    .article-summary {
      margin: 0;
      font-size: 12px;
      color: #7fa3c9;
      line-height: 1.5;
    }
  `]
})
export class NewsPanelComponent implements OnInit {
    articles: Article[] = [];
    loading = false;
    error = '';

    constructor(private api: ApiService) { }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.loading = true;
        this.error = '';
        this.api.getArticles(50).subscribe({
            next: (data) => {
                this.articles = data;
                this.loading = false;
            },
            error: (err) => {
                this.error = 'Failed to load articles';
                this.loading = false;
            }
        });
    }
}
