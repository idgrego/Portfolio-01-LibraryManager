import { Component } from '@angular/core';
import { RouterOutlet } from "@angular/router";
import { HeaderMenuComponent } from './components/header-menu/header-menu.component';
import { NgClass } from "@angular/common";
import { DialogConfirmComponent } from './components/dialog-confirm/dialog-confirm.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderMenuComponent, NgClass, DialogConfirmComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  
  title = 'LibraryManager-UI';

  get bgImageClass() {
    const path = window.location.pathname
    if(path.includes('/authors'))
      return 'bg-authors'
    else if(path.includes('/books'))
      return 'bg-books'
    else
      return 'bg-not-found'
  }

}
