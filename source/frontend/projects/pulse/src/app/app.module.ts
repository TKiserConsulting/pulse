import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CoreModule } from './shared/modules/core.module';
import { SharedModule } from './shared/modules/shared.module';
import { WindowRefService } from './shared/providers/window.provider';
import { MainModule } from './modules/main/main.module';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        CoreModule,
        SharedModule,
        AppRoutingModule,
        MainModule,
    ],
    declarations: [AppComponent],
    providers: [WindowRefService],
    bootstrap: [AppComponent],
})
export class AppModule {}
