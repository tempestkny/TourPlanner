import { Component, OnInit } from '@angular/core';
import { Statistics, StatisticsService } from '../statistics.service';

@Component({
  selector: 'app-statistics-dashboard',
  templateUrl: './statistics-dashboard.html',
  styleUrl: './statistics-dashboard.css',
})
export class StatisticsDashboard implements OnInit {
  statistics: Statistics | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(private readonly statisticsService: StatisticsService) {}

  ngOnInit(): void {
    this.loadStatistics();
  }

  loadStatistics(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.statisticsService.getStatistics().subscribe({
      next: (statistics) => {
        this.statistics = statistics;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load statistics: ', error);
        this.errorMessage = 'Could not load statistics.';
        this.isLoading = false;
      }
    });
  }
}
