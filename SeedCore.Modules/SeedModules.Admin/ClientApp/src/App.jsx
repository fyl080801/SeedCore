import React from 'react';
import { Row, Col, Card } from 'antd';

import 'bootstrap/dist/css/bootstrap-grid.css';
import 'antd/dist/antd.css';
import './App.css';

export default () => {
  return (
    <div>
      <h1>welcom</h1>
      <Row gutter={16}>
        <Col span={6}>
          <Card>
            <p>text...</p>
            <p>text...</p>
            <p>text...</p>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <p>text...</p>
            <p>text...</p>
            <p>text...</p>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <p>text...</p>
            <p>text...</p>
            <p>text...</p>
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <p>text...</p>
            <p>text...</p>
            <p>text...</p>
          </Card>
        </Col>
      </Row>
    </div>
  );
};
