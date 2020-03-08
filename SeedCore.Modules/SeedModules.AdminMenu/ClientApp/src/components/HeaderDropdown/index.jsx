import React from 'react';
import classNames from 'classnames';
import Dropdown from 'antd/es/dropdown';
import styles from './index.less';

export default ({ overlayClassName: cls, ...restProps }) => {
  return (
    <Dropdown
      overlayClassName={classNames(styles.container, cls)}
      {...restProps}
    />
  );
};
