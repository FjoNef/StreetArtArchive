import React, {Component, useCallback, useState} from 'react';
import {Card, CardBody, CardText, CardTitle, Col, Row} from 'reactstrap';
import InfiniteScroll from 'react-infinite-scroller';

export class FetchData extends Component {
  static displayName = FetchData.name;  

  constructor(props) {
    super(props);
    this.state = {forecasts: [], hasMore: true, loading: false};
    this.populateWeatherData = this.populateWeatherData.bind(this);
  }
  
  async fetchData(){
      if(this.state.loading){
          return;
      }

      this.setState({ loading: true });

      try{

          const response = await fetch('weatherforecast?page='+0);
          const data = await response.json();
          this.setState( { forecasts: [...this.state.forecasts, ...data.pictures], hasMore: data.hasMore})
      }
      finally {
          this.setState({ loading: false });
      }
  }

    async populateWeatherData(page) {
        if(this.state.loading){
            return;
        }

        this.setState({ loading: true });

        try{

            const response = await fetch('weatherforecast?page='+page);
            const data = await response.json();
            this.setState( { forecasts: [...this.state.forecasts, ...data.pictures], hasMore: data.hasMore})
        }
        finally {
            this.setState({ loading: false });
        }
    }

  render() {
    return (
      <div>
        <h1 id="tableLabel">Weather forecast</h1>
        
          <InfiniteScroll
              pageStart={0}
              loadMore={this.populateWeatherData}
              hasMore={this.state.hasMore}
              loader={<div className="loader" key={0}>Loading ...</div>}
          >
              <Row xs={3}>
            {this.state.forecasts.map(forecast =>
                <Col>
                  <Card>
                    <CardBody>
                      <CardTitle tag="h5">
                        {forecast.date}
                      </CardTitle>
                      <CardText>
                        {forecast.temperatureC} {forecast.temperatureF} {forecast.summary}
                      </CardText>
                    </CardBody>
                  </Card>
                </Col>
            )}
              </Row>
          </InfiniteScroll>
        
      </div>
    );
  }
}
