import React, { useState } from 'react';
import { Typography, Button, Form, Input, Select, Row, Col } from 'antd';
import 'bootstrap/dist/css/bootstrap-grid.css';
import 'antd/dist/antd.css';

export default () => {
  const [name, setName] = useState('');

  return (
    <div className="container">
      <div className="well">well !!</div>
      <Form labelCol={120}>
        <Row gutter={24}>
          <Col span={24}>
            <Typography.Title level={3}>基本信息</Typography.Title>
          </Col>
          <Col span={12}>
            <Form.Item label="名称:">
              <Input
                placeholder="输入名称"
                value={name}
                onChange={value => {
                  setName(value.target.value);
                }}
              ></Input>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="名称:">
              <Input.Group compact>
                <Select placeholder="请选择" style={{ width: '50%' }}></Select>
                <Button icon="folder-open">打开111</Button>
              </Input.Group>
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>数据库</Typography.Title>
          </Col>
          <Col span={12}>
            <Form.Item label="类型:">
              <Select placeholder="请选择"></Select>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="表前缀:">
              <Input placeholder="请选择"></Input>
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>账号信息</Typography.Title>
          </Col>
          <Col span={12}>
            <Form.Item label="用户名:">
              <Input placeholder="输入名称"></Input>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="邮箱:">
              <Input placeholder="输入名称"></Input>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="密码:">
              <Input.Password
                placeholder="输入名称"
                visibilityToggle={false}
              ></Input.Password>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="确认密码:">
              <Input.Password
                placeholder="输入名称"
                visibilityToggle={false}
              ></Input.Password>
            </Form.Item>
          </Col>
        </Row>
      </Form>
      <Button type="primary">开始安装</Button>
    </div>
  );
};
