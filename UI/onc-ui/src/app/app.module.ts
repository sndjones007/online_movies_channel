import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SidebarComponent } from './ui/sidebar/sidebar.component';
import { LayoutComponent } from './ui/layout/layout.component';
import { MainContentComponent } from './ui/main-content/main-content.component';
import { ContactComponent } from './ui/contact/contact.component';
import { AcademyAwardsComponent } from './ui/academy-awards/academy-awards.component';
import { AboutComponent } from './ui/about/about.component';

@NgModule({
  declarations: [
    AppComponent,
    SidebarComponent,
    LayoutComponent,
    MainContentComponent,
    ContactComponent,
    AcademyAwardsComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
