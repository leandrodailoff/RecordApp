import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Note } from '../models/note.model';
import { environment } from '../environments/environment';

const CACHE_KEY = 'recordapp_notes';
const CACHE_TTL_MS = 5 * 60 * 1000; // 5 minutes

interface NoteCache {
  notes: Note[];
  cachedAt: number;
}

@Injectable({ providedIn: 'root' })
export class NoteService {
  private readonly apiUrl = `${environment.apiUrl}/notes`;
  private http = inject(HttpClient);

  // Returns cached notes if they exist and are fresh, otherwise null
  getFromCache(): Note[] | null {
    try {
      const raw = localStorage.getItem(CACHE_KEY);
      if (!raw) return null;
      const cache: NoteCache = JSON.parse(raw);
      const isExpired = Date.now() - cache.cachedAt > CACHE_TTL_MS;
      return isExpired ? null : cache.notes;
    } catch {
      return null;
    }
  }

  private setCache(notes: Note[]): void {
    const cache: NoteCache = { notes, cachedAt: Date.now() };
    localStorage.setItem(CACHE_KEY, JSON.stringify(cache));
  }

  clearCache(): void {
    localStorage.removeItem(CACHE_KEY);
  }

  getAll(): Observable<Note[]> {
    return this.http.get<Note[]>(this.apiUrl).pipe(
      tap(notes => this.setCache(notes))
    );
  }

  create(note: Note): Observable<Note> {
    return this.http.post<Note>(this.apiUrl, note);
  }

  update(note: Note): Observable<Note> {
    return this.http.put<Note>(`${this.apiUrl}/${note.id}`, note);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
