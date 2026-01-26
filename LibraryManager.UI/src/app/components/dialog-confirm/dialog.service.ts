import { Injectable } from "@angular/core";
import { Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class DialogService {
    title: string = ''
    message: string = ''
    confirmText: string = ''
    cancelText: string = ''

    isNotification: boolean = false
    isVisible: boolean = false

    notify$ = new Subject<boolean>()

    notify(result: boolean) {
        this.isVisible = false
        this.notify$.next(result)
    }

    show(data: DialogData) {
        this.title = data.title
        this.message = data.message
        this.confirmText = data.confirmText
        this.isNotification = data.isNotification
        this.isVisible = true
        if (data.cancelText) this.cancelText = data.cancelText
    }

    showError(data: DialogErrorData, err: any, printConsole = true) {
        
        const message = data.message ?? err?.error?.detail ?? err?.message

        this.show({
            title: data.title,
            isNotification: true,
            confirmText: '',
            cancelText: data.cancelText ?? 'Got it',
            message: (message?.length > 1000 ? message.substring(0, 1000) + '...' : message),
          })

        if (printConsole) 
            console.error(data.title, err);
    }
}

export interface DialogData {
    title: string
    message: string
    confirmText: string
    isNotification: boolean
    cancelText?: string
}

export interface DialogErrorData {
    title: string
    isNotification?: boolean
    confirmText?: string
    cancelText?: string
    message?: string

}