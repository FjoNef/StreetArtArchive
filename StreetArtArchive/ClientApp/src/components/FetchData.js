import React, { Component } from 'react';
import {Card, CardBody, CardText, CardTitle, Col, Row} from 'reactstrap';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { forecasts: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(forecasts) {
    return (
      <Row xs={3}>
        {forecasts.map(forecast =>
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
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.forecasts);

    return (
      <div>
        <h1 id="tableLabel">Weather forecast</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('weatherforecast');
    const data = await response.json();
    this.setState({ forecasts: data, loading: false });
  }
}
