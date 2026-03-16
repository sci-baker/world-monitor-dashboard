export interface Article {
  id: number;
  title: string;
  summary: string;
  url: string;
  imageUrl: string | null;
  author: string | null;
  category: string;
  sourceName: string;
  publishedAt: string;
}

export interface MapEvent {
  id: number;
  title: string;
  description: string;
  latitude: number;
  longitude: number;
  category: string;
  severity: string;
  sourceUrl: string | null;
  eventDate: string;
}

export interface Brief {
  id: number;
  title: string;
  content: string;
  model: string;
  articleCount: number;
  generatedAt: string;
}

export interface IngestResult {
  message: string;
}
