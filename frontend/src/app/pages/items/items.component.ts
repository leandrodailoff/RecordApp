import { Component } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule],
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.scss'],
})
export class ItemsComponent {
  listId: number | null = null;
  items = ['Item 1', 'Item 2'];
  newItem = '';

  constructor(private route: ActivatedRoute) {
    this.listId = Number(this.route.snapshot.paramMap.get('listId'));
  }

  addItem() {
    if (this.newItem.trim()) {
      this.items.push(this.newItem.trim());
      this.newItem = '';
    }
  }
}
