import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ContactComponent } from './ui/contact/contact.component';
import { AcademyAwardsComponent } from './ui/academy-awards/academy-awards.component';
import { AboutComponent } from './ui/about/about.component';

const routes: Routes = [
  { path: 'oscar', component: AcademyAwardsComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'about', component: AboutComponent },
  { path: '', redirectTo: '/oscar', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
