import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { CommonModule } from '@angular/common';
import * as L from 'leaflet';
import { ApiService } from '../../services/api.service';
import { MapEvent } from '../../models/api.models';

@Component({
  selector: 'app-map-panel',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="panel">
      <div class="panel-header">
        <h2>🌍 Event Map</h2>

        <div class="toolbar">
          <select
            class="filter"
            [value]="selectedCategory"
            (change)="onCategoryChange($any($event.target).value)">
            <option value="">All categories</option>
            <option *ngFor="let category of categories" [value]="category">
              {{ category }}
            </option>
          </select>

          <span class="count">{{ events.length }} mapped</span>
        </div>
      </div>

      <div class="legend">
        <span><i class="dot critical"></i> critical</span>
        <span><i class="dot high"></i> high</span>
        <span><i class="dot medium"></i> medium</span>
        <span><i class="dot low"></i> low</span>
      </div>

      <div class="loading" *ngIf="loading">Loading map data…</div>
      <div class="error" *ngIf="error">⚠ {{ error }}</div>

      <div #mapContainer class="map-container"></div>
    </div>
  `,
  styles: [`
    .panel {
      background: #0f1923;
      border: 1px solid #1e2d3d;
      border-radius: 6px;
      padding: 16px;
      display: flex;
      flex-direction: column;
      height: 100%;
    }

    .panel-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 12px;
      margin-bottom: 10px;
      flex-wrap: wrap;
    }

    h2 {
      margin: 0;
      font-size: 15px;
      color: #c8d6e5;
      font-weight: 600;
    }

    .toolbar {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .filter {
      background: #122231;
      color: #dbe7f3;
      border: 1px solid #284055;
      border-radius: 4px;
      padding: 6px 10px;
      font-size: 12px;
    }

    .count {
      font-size: 12px;
      color: #7f9bb8;
      white-space: nowrap;
    }

    .legend {
      display: flex;
      gap: 14px;
      flex-wrap: wrap;
      margin-bottom: 10px;
      font-size: 12px;
      color: #90a4b7;
    }

    .dot {
      display: inline-block;
      width: 10px;
      height: 10px;
      border-radius: 50%;
      margin-right: 6px;
      vertical-align: middle;
    }

    .critical { background: #e74c3c; }
    .high { background: #e67e22; }
    .medium { background: #f1c40f; }
    .low { background: #2ecc71; }

    .loading, .error {
      font-size: 13px;
      text-align: center;
      margin-bottom: 8px;
    }

    .loading { color: #7f9bb8; }
    .error { color: #e74c3c; }

    .map-container {
      flex: 1;
      min-height: 360px;
      border-radius: 6px;
      overflow: hidden;
    }
  `]
})
export class MapPanelComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapContainer', { static: true })
  mapContainer!: ElementRef<HTMLDivElement>;

  events: MapEvent[] = [];
  categories: string[] = [];
  selectedCategory = '';
  loading = false;
  error = '';

  private map?: L.Map;
  private markersLayer = L.layerGroup();
  private readonly defaultCenter: L.LatLngTuple = [20, 0];
  private readonly defaultZoom = 2;

  constructor(private api: ApiService) {}

  ngAfterViewInit(): void {
    this.initMap();
    this.loadCategories();
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }

  onCategoryChange(category: string): void {
    this.selectedCategory = category;
    this.loadEvents(category || undefined);
  }

  private initMap(): void {
    this.map = L.map(this.mapContainer.nativeElement, {
      center: this.defaultCenter,
      zoom: this.defaultZoom,
      zoomControl: true,
      worldCopyJump: true
    });

    L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
      attribution: '&copy; OpenStreetMap &copy; CARTO',
      maxZoom: 18
    }).addTo(this.map);

    this.markersLayer.addTo(this.map);

    setTimeout(() => {
      this.map?.invalidateSize();
    }, 0);
  }

  private loadCategories(): void {
    this.api.getMapCategories().subscribe({
      next: (data) => {
        this.categories = data ?? [];
      },
      error: () => {
        this.categories = [];
      }
    });
  }

  private loadEvents(category?: string): void {
    this.loading = true;
    this.error = '';

    this.api.getMapEvents(category).subscribe({
      next: (data) => {
        this.events = (data ?? []).filter(
          e => Number.isFinite(e.latitude) && Number.isFinite(e.longitude)
        );
        this.loading = false;
        this.renderMarkers();
      },
      error: () => {
        this.error = 'Failed to load map events';
        this.loading = false;
        this.events = [];
        this.renderMarkers();
      }
    });
  }

  private renderMarkers(): void {
    if (!this.map) return;

    this.markersLayer.clearLayers();

    const points: L.LatLngTuple[] = [];

    for (const ev of this.events) {
      const color = this.getSeverityColor(ev.severity);

      const marker = L.circleMarker([ev.latitude, ev.longitude], {
        radius: 7,
        color: '#ffffff',
        weight: 1,
        fillColor: color,
        fillOpacity: 0.9
      });

      marker.bindPopup(this.buildPopup(ev));
      marker.addTo(this.markersLayer);

      points.push([ev.latitude, ev.longitude]);
    }

    if (points.length === 1) {
      this.map.setView(points[0], 5);
    } else if (points.length > 1) {
      this.map.fitBounds(points, { padding: [30, 30] });
    } else {
      this.map.setView(this.defaultCenter, this.defaultZoom);
    }
  }

  private getSeverityColor(severity?: string | null): string {
    switch ((severity ?? '').toLowerCase()) {
      case 'critical': return '#e74c3c';
      case 'high': return '#e67e22';
      case 'medium': return '#f1c40f';
      case 'low': return '#2ecc71';
      default: return '#3498db';
    }
  }

  private buildPopup(ev: MapEvent): string {
    const title = this.escapeHtml(ev.title);
    const description = this.escapeHtml(ev.description ?? '');
    const category = this.escapeHtml(ev.category ?? 'uncategorized');
    const severity = this.escapeHtml(ev.severity ?? 'unknown');
    const date = new Date(ev.eventDate).toLocaleString();

    const safeUrl = this.safeUrl(ev.sourceUrl);
    const sourceLink = safeUrl
      ? `<div style="margin-top:8px"><a href="${safeUrl}" target="_blank" rel="noopener noreferrer">Open source</a></div>`
      : '';

    return `
      <div style="font-size:13px; max-width:240px; line-height:1.4;">
        <strong>${title}</strong><br />
        <span style="color:#8aa0b5; font-size:11px;">
          ${category} · ${severity} · ${date}
        </span>
        ${description ? `<p style="margin:8px 0 0;">${description}</p>` : ''}
        ${sourceLink}
      </div>
    `;
  }

  private safeUrl(value?: string | null): string | null {
    if (!value) return null;

    try {
      const url = new URL(value);
      if (url.protocol === 'http:' || url.protocol === 'https:') {
        return url.toString();
      }
      return null;
    } catch {
      return null;
    }
  }

  private escapeHtml(value: string): string {
    return value
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }
}