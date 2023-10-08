import React, {Component} from 'react';
import {Card, CardBody, CardText, CardTitle, Col, Row} from 'reactstrap';
import InfiniteScroll from 'react-infinite-scroller';

export class FetchData extends Component {
  static displayName = FetchData.name;  

  constructor(props) {
    super(props);
    this.state = {pictures: [], page: 0, hasMore: true, loading: false};
    this.populatePictures = this.populatePictures.bind(this);
  }

    async populatePictures() {
      if(this.state.loading){
        return;
      }

      this.setState({ loading: true });

      try{
        const response = await fetch('pictures/GetList?page='+this.state.page);
        const data = await response.json();
        this.setState( { 
          pictures: [...this.state.pictures, ...data.pictures], 
          page: this.state.page+1, 
          hasMore: data.hasMore})
      }
      catch (e){
        this.setState({ hasMore: false });
      }
      finally {
        this.setState({ loading: false });
      }
    }

  render() {
    return (
      <div>
        <h1 id="tableLabel">Pictures</h1>
        
          <InfiniteScroll
              loadMore={this.populatePictures}
              hasMore={this.state.hasMore}
              loader={<div className="loader" key={0}>Loading ...</div>}
          >
              <Row xs={3}>
                {this.state.pictures.map(picture =>
                    <Col>
                      <Card body>
                        <img
                          alt= "No Thumbnail"
                          src = {"data:image/png;base64," + picture.thumbnail.data}
                          width="100%"
                        />
                        <CardBody>
                          <CardTitle tag="h5">
                            {picture.categories.find(c => c.name === "Name")?.values[0]}
                          </CardTitle>
                          <CardText>
                            {picture.categories.find(c => c.name === "Year")?.values[0]}
                            {picture.categories.find(c => c.name === "Author")?.values}
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
