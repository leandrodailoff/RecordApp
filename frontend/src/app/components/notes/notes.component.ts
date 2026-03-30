import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NoteService } from '../../services/note.service';
import { Note } from '../../models/note.model';

@Component({
  selector: 'app-notes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css'
})
export class NotesComponent implements OnInit {
  noteService = inject(NoteService);
  private cdr = inject(ChangeDetectorRef);

  notes: Note[] | null = null;
  newNote: Note = { title: '', content: '', color: '#fef08a' };
  editingNote: Note | null = null;
  showForm = false;
  saving = false;

  searchQuery = '';
  sortBy: 'date' | 'title' | 'color' = 'date';

  palette = ['#fef08a','#86efac','#93c5fd','#f9a8d4','#fca5a5','#d8b4fe','#fed7aa','#e2e8f0'];
  rotations = ['-1.5deg','1deg','-0.5deg','2deg','-2deg','0.8deg','-1.2deg','1.8deg'];

  get filteredNotes(): Note[] {
    if (!this.notes) return [];

    let result = [...this.notes];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      result = result.filter(n =>
        n.title.toLowerCase().includes(q) ||
        n.content.toLowerCase().includes(q)
      );
    }

    switch (this.sortBy) {
      case 'title':
        result.sort((a, b) => a.title.localeCompare(b.title));
        break;
      case 'color':
        result.sort((a, b) => a.color.localeCompare(b.color));
        break;
      case 'date':
      default:
        result.sort((a, b) =>
          new Date(b.createdAt!).getTime() - new Date(a.createdAt!).getTime()
        );
        break;
    }

    return result;
  }

  ngOnInit(): void {
    this.noteService.checkColdStart();

    const cached = this.noteService.getFromCache();
    if (cached) {
      this.notes = cached;
      this.cdr.detectChanges();
    }

    this.noteService.getAll().subscribe(notes => {
      this.notes = [...notes];
      this.cdr.detectChanges();
    });
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) this.newNote = { title: '', content: '', color: '#fef08a' };
  }

  createNote(): void {
    if (!this.newNote.title.trim() || this.saving) return;
    this.saving = true;
    this.noteService.create(this.newNote).subscribe(created => {
      this.notes = [created, ...(this.notes ?? [])];
      this.noteService.clearCache();
      this.newNote = { title: '', content: '', color: '#fef08a' };
      this.showForm = false;
      this.saving = false;
      this.cdr.detectChanges();
    });
  }

  deleteNote(id: number): void {
    this.notes = (this.notes ?? []).filter(n => n.id !== id);
    this.noteService.clearCache();
    this.noteService.delete(id).subscribe();
    this.cdr.detectChanges();
  }

  startEdit(note: Note): void { this.editingNote = { ...note }; }
  cancelEdit(): void { this.editingNote = null; }

  saveEdit(): void {
    if (!this.editingNote || !this.editingNote.title.trim() || this.saving) return;
    this.saving = true;
    const updated = { ...this.editingNote };
    this.noteService.update(updated).subscribe(saved => {
      this.notes = (this.notes ?? []).map(n => n.id === saved.id ? saved : n);
      this.noteService.clearCache();
      this.editingNote = null;
      this.saving = false;
      this.cdr.detectChanges();
    });
  }
}
