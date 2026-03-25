import { Component } from '@angular/core';
import { NotesComponent } from '../notes/notes.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NotesComponent],
  template: `<app-notes />`
})
export class HomeComponent {}
