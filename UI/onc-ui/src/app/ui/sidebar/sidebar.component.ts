import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  is_sub_menu: boolean = true;
  year_menu_item: number;
  award_years: number[];
  
  constructor() { }

  ngOnInit() {
    this.award_years = [1928, 1929, 1930, 1931];
    this.year_menu_item = 1927;
  }

  onDisplayDecades() {

  }

  onDisplayOscarItem(year: number) {

  }

}
