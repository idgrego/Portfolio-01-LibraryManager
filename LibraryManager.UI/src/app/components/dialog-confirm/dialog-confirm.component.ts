import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DialogService } from './dialog.service';

@Component({
  selector: 'app-dialog-confirm',
  standalone: true,
  imports: [],
  templateUrl: './dialog-confirm.component.html',
  styleUrl: './dialog-confirm.component.scss'
})
export class DialogConfirmComponent {
  get title() { return this.dialogService.title; }
  get message() { return this.dialogService.message; }
  get confirmText() { return this.dialogService.confirmText; }
  get cancelText() { return this.dialogService.cancelText; }
  get isNotification() { return this.dialogService.isNotification; }
  get isVisible() { return this.dialogService.isVisible; }

  onCancel() { this.dialogService.notify(false) }
  onConfirm() { this.dialogService.notify(true) }

  constructor(private dialogService: DialogService) {}
}
