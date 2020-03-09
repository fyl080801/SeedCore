import React from 'react';
import Avatar from 'antd/es/avatar';
import Menu from 'antd/es/menu';
import Modal from 'antd/es/modal';
import {
  LogoutOutlined,
  QuestionCircleFilled,
  SettingOutlined,
  UserOutlined
} from '@ant-design/icons';
import HeaderDropdown from '../HeaderDropdown';
import styles from './index.less';
import { logout } from '../../api/account';

console.log(styles);

export default props => {
  const { menu } = props;

  const onMenuClick = () => {};

  const onLogout = () => {
    Modal.confirm({
      title: '提示',
      content: '是否退出？',
      okText: '是',
      cancelText: '否',
      icon: <QuestionCircleFilled />,
      onOk: () => {
        logout();
      }
    });
  };

  const menuHeaderDropdown = (
    <Menu className={styles.menu} selectedKeys={[]} onClick={onMenuClick}>
      {menu && <Menu.Item key="center">个人中心</Menu.Item>}
      {menu && <Menu.Item key="settings">个人设置</Menu.Item>}
      {menu && <Menu.Divider />}
      <Menu.Item key="logout" onClick={onLogout}>
        <LogoutOutlined />
        退出登录
      </Menu.Item>
    </Menu>
  );

  return (
    <HeaderDropdown overlay={menuHeaderDropdown}>
      <span className={`${styles.action} ${styles.account}`}>
        <Avatar size="small" className={styles.avatar} alt="avatar" />
        <span className={styles.name}>用户名</span>
      </span>
    </HeaderDropdown>
  );
};
