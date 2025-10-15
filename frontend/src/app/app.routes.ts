import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { ListsComponent } from './pages/lists/lists.component';
import { ItemsComponent } from './pages/items/items.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'lists', component: ListsComponent },
  { path: 'items/:listId', component: ItemsComponent },
];
