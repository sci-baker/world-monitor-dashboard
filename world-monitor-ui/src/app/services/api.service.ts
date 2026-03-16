import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Article, Brief, IngestResult, MapEvent } from '../models/api.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = (environment.apiUrl ?? '').replace(/\/+$/, '');

  constructor(private readonly http: HttpClient) {}

  // -----------------------
  // News
  // -----------------------

  getArticles(
    count: number = 50,
    source?: string,
    category?: string
  ): Observable<Article[]> {
    let params = new HttpParams().set('count', String(count));

    if (source?.trim()) {
      params = params.set('source', source.trim());
    }

    if (category?.trim()) {
      params = params.set('category', category.trim());
    }

    return this.http.get<Article[]>(`${this.baseUrl}/api/news`, { params });
  }

  triggerIngest(): Observable<IngestResult> {
    return this.http.post<IngestResult>(`${this.baseUrl}/api/news/ingest`, {});
  }

  // -----------------------
  // Map
  // -----------------------

  getMapEvents(category?: string): Observable<MapEvent[]> {
    let params = new HttpParams();

    if (category?.trim()) {
      params = params.set('category', category.trim());
    }

    return this.http.get<MapEvent[]>(`${this.baseUrl}/api/map`, { params });
  }

  getMapCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/api/map/categories`);
  }

  getMapEventById(id: number): Observable<MapEvent> {
    return this.http.get<MapEvent>(`${this.baseUrl}/api/map/${id}`);
  }

  createMapEvent(payload: Partial<MapEvent>): Observable<MapEvent> {
    return this.http.post<MapEvent>(`${this.baseUrl}/api/map`, payload);
  }

  // -----------------------
  // Brief
  // -----------------------

  getLatestBrief(): Observable<Brief> {
    return this.http.get<Brief>(`${this.baseUrl}/api/brief`);
  }

  generateBrief(): Observable<Brief> {
    return this.http.post<Brief>(`${this.baseUrl}/api/brief/generate`, {});
  }
}